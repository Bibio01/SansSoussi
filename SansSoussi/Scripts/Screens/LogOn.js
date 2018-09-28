
$(document).ready(function () {
    var storage = window.localStorage;
    var getUserName = storage["username"];
    var getPassword = storage["password"];
    var getIsAutoLogin = storage["isAutoLogin"];
    var getIsStorePwd = storage["isStorePwd"];
    if (getIsStorePwd == "yes") {
        if (getIsAutoLogin == "yes") {
            if ((getUserName != "" || getUserName != null) && (getPassword != "" || getPassword != null)) {
                $("#UserName").val(getUserName);
                $("#Password").val(getPassword);

                $.ajax({
                    url: ResolveUrl("~/Home/Search"),
                    type: "POST",
                    data: {
                        username: getUserName,
                        password: getPassword
                    },
                    success: function (data) {
                        alert("good");
                    },
                    error: function () {
                        alert("wrong");
                    }
                });
            }
        } else {
            $("#UserName").val(getUserName);
            $("#Password").val(getPassword);
        }
    }
});

function LogOn() {
    //var username = $("UserName").val();
    //var password = $("PassWord").val();
    var storage = window.localStorage;
    //remember pwd
    
    if (document.getElementById('RememberMe').checked) {
        storage.setItem("username", document.getElementById('UserName').value);
        storage.setItem("password", document.getElementById('Password').value);
        storage["isStorePwd"] = "yes";
    } else {
        storage.setItem("username", document.getElementById('UserName').value);
        storage["isStorePwd"] = "no";
    }
    
    //auto-connection next time
    
    if (document.getElementById('isAutoLoginId').checked) {
        storage.setItem("username", document.getElementById('UserName').value);
        storage.setItem("password", document.getElementById('Password').value);
        storage["isStorePwd"] = "yes";
        storage["isAutoLogin"] = "yes";
    } else {
        storage.setItem("username", document.getElementById('UserName').value);
        storage["isAutoLogin"] = "no";
    }
    
    $.ajax({
        url: ResolveUrl("~/Account/LogOn"),
        type: "POST",
        data: {
            username: username,
            password: password
        },
        success: function (data) {
            window.location.href = ResolveUrl("~/Home/Search");
        }
    })

}