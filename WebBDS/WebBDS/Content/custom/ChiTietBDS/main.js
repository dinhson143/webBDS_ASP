var btnThuemua = document.querySelector(".search2_title--thuemua")
var btnKygui = document.querySelector(".search2_title--kigui")
btnThuemua.onclick = function()
{
    document.querySelector(".blockThuemua").style.display="block";
    document.querySelector(".blockKiGui").style.display="none";
}
btnKygui.onclick =function()
{
    document.querySelector(".blockKiGui").style.display="block";
    document.querySelector(".blockThuemua").style.display="none";
}