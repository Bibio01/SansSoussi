using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
using Microsoft.Security.Application;

namespace SansSoussi.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection _dbConnection;
        public HomeController()
        {
             _dbConnection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString);
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Parce que marcher devrait se faire SansSoussi";

            return View();
        }

        public ActionResult Comments()
        {
            List<string> comments = new List<string>();

            //Get current user from default membership provider
            MembershipUser user = Membership.Provider.GetUser(HttpContext.User.Identity.Name, true);
            if (user != null)
            {
                string query = "Select Comment from Comments where UserId = @userId";
                SqlCommand cmd = new SqlCommand(query, _dbConnection);
                cmd.Parameters.AddWithValue("@userId", user.ProviderUserKey);
                _dbConnection.Open();
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    comments.Add(rd.GetString(0));
                }

                rd.Close();
                _dbConnection.Close();
            }
            return View(comments);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Comments(string comment)
        {
            string EncodedComment = Encoder.HtmlEncode(comment);
            try
            {
                //Get current user from default membership provider
                MembershipUser user = Membership.Provider.GetUser(HttpContext.User.Identity.Name, true);
                if (user != null)
                {
                    //add new comment to db
                    string query = "insert into Comments (UserId, CommentId, Comment) Values (@userId, @guid, @encodedComment)";
                    SqlCommand cmd = new SqlCommand(query, _dbConnection);
                    cmd.Parameters.AddWithValue("@userId", user.ProviderUserKey);
                    cmd.Parameters.AddWithValue("@guid", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@encodedComment", EncodedComment);
                    _dbConnection.Open();

                    cmd.ExecuteNonQuery();
                    _dbConnection.Close();
                }
                else
                {
                    throw new Exception("Vous devez vous connecter");
                }
            }
            catch (Exception ex)
            {
                _dbConnection.Close();
                throw new Exception(ex.Message);
            }

            return Comments();
        }

        [ValidateInput(false)]
        public ActionResult Search(string searchData)
        {
            
            List<string> searchResults = new List<string>();

            //Get current user from default membership provider
            
            MembershipUser user = Membership.Provider.GetUser(HttpContext.User.Identity.Name, true);
            
            if (user != null)
            {
                if (!string.IsNullOrEmpty(searchData))
                {
                    string encodedSearchData = Encoder.HtmlEncode(searchData);
                    string query = "Select Comment from Comments where UserId = @userId and Comment like @encodedSearchData";
                    SqlCommand cmd = new SqlCommand(query, _dbConnection);
                    cmd.Parameters.AddWithValue("@userId", user.ProviderUserKey);
                    
                    cmd.Parameters.AddWithValue("@encodedSearchData", '%' + encodedSearchData + '%');
                    _dbConnection.Open();
                    SqlDataReader rd = cmd.ExecuteReader();


                    while (rd.Read())
                    {
                        searchResults.Add(rd.GetString(0));
                    }

                    rd.Close();
                    _dbConnection.Close();
                }
            }
            return View(searchResults);
        }

        [HttpGet]
        public ActionResult Emails()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Emails(object form)
        {
            List<string> searchResults = new List<string>();

            HttpCookie cookie = HttpContext.Request.Cookies["username"];
            
            List<string> cookieString = new List<string>();

            //Decode the cookie from base64 encoding
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(cookie.Value);
            string strCookieValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            //get user role base on cookie value
            string[] roles = Roles.GetRolesForUser(strCookieValue);

            bool isAdmin = roles.Contains("admin");

            if (isAdmin)
            {
                SqlCommand cmd = new SqlCommand("Select Email from aspnet_Membership", _dbConnection);
                _dbConnection.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    searchResults.Add(rd.GetString(0));
                }
                rd.Close();
                _dbConnection.Close();
            }


            return Json(searchResults);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
