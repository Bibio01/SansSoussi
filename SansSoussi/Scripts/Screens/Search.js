window.onbeforeunload = confirmeExit;
function confirmeExit() {
    var storage = window.localStorage;
    storage.setItem("inputString", document.getElementById('searchBox').value);
    
}
function search() {
    window.location = ResolveUrl("~/home/Search?searchData=" + $("#searchBox").val());
    var storage = window.localStorage;
    //storage.setItem("inputString",document.getElementById('searchBox').value);
    
}
$(document).ready(function () {
    var storage = window.localStorage;
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