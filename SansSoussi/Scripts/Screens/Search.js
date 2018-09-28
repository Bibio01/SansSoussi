window.onbeforeunload = confirmeExit;
function confirmeExit() {
    var storage = window.sessionStorage;
    storage.setItem("inputString", document.getElementById('searchBox').value);
    alert(1);
}
function search() {
    window.location = ResolveUrl("~/home/Search?searchData=" + $("#searchBox").val());
    var storage = window.sessionStorage;
    //storage.setItem("inputString",document.getElementById('searchBox').value);
    
}
$(document).ready(function () {
    var storage = window.sessionStorage;
    var input = storage["inputString"];
    
    $.ajax({
        url: ResolveUrl("~/Home/Search"),
        type: "POST",
        data: {
            input: input
        },
        success: function (data) {
            //storage.setItem("inputString", document.getElementById('searchBox').value);
            $("#searchBox").val(input);
        }
    });
})