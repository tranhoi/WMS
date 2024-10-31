//focus vao element textbox và xóa data
function FocusElementText(objId) {
   
    if (document.getElementById(objId) == null) return;

    var _element = document.getElementById(objId);

    _element.value = "";

    _element.focus();
}