/// <reference path="../jquery-1.7.1-vsdoc.js" />

function AddComments() {
    if ($("#NewComment:visible").length < 1) {
        $("#NewComment").show();
        $("#NewCommentsBtn").val("Ajouter");
    }
    else {
        $.ajax({
            url: ResolveUrl("~/home/comments"),
            type: "POST",
            //data: { comment: htmlEncode($("#NewComment").val()) },
            data: { comment: $("#NewComment").val() },
            success: function (status) {
                if (status != "success") {
                    alert(status);
                }
                
                $("#NewComment").hide();
                $("#NewCommentsBtn").val("Nouveau commentaire");
            },
            error: function (info) {
                alert(info);
            }
        }
        );
    }
}

function htmlEncode(str) {
    var ele = document.createElement('span');
    ele.appendChild(document.createTextNode(str));
    return ele.innerHTML;
}
function htmlDecode(str) {
    var ele = document.createElement('span');
    ele.innerHTML = str;
    return ele.textContent;
}
function addNewComment() {
    
}