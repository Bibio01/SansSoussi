﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.Security.Application;
using SansSoussi.Models;
using SecurityEssentials.Core.Attributes;

namespace SansSoussi.Controllers
{
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowXRequestsEveryXSecondsAttribute(Name = "LogOn", Message = "You have performed this action more than {x} times in the last {n} seconds.", Requests = 5, Seconds = 60)]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        //SHA512(SHA512(password)+salt) NO
                        //SHA512(password)
                        byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(model.UserName);
                        byte[] result_mid;
                        byte[] result;

                        SHA1 shaM = new SHA1Managed();
                        result_mid = shaM.ComputeHash(toEncodeAsBytes);
                        /*
                        var salt = Guid.NewGuid().ToString();
                        String result_mid_str = result_mid + salt;
                        byte[] array_mid = System.Text.ASCIIEncoding.ASCII.GetBytes(result_mid_str);
                        result = shaM.ComputeHash(array_mid);
                        */
                        HttpCookie authCookie = new HttpCookie("username",System.Text.ASCIIEncoding.ASCII.GetString(result_mid));
                        authCookie.Expires = DateTime.Now.AddDays(-1d);
                        HttpContext.Response.Cookies.Add(authCookie);
                        
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
               
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************
        
        public ActionResult LogOff()
        {
            FormsService.SignOut();

            //HttpContext.Response.Cookies.Clear();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {

            model.Password = Encoder.HtmlEncode(model.Password);
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {

                    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(model.UserName);
                    byte[] result;

                    SHA1 shaM = new SHA1Managed();
                    result = shaM.ComputeHash(toEncodeAsBytes);
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    //Encode the username in base64

                    HttpCookie authCookie = new HttpCookie("username", System.Text.ASCIIEncoding.ASCII.GetString(result));
                    HttpContext.Response.Cookies.Add(authCookie);

                    //byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(model.UserName);
                    //HttpCookie authCookie = new HttpCookie("username", System.Convert.ToBase64String(toEncodeAsBytes));
                    
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }
}
