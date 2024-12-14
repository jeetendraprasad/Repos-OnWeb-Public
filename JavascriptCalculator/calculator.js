var input_box = document.getElementById("calculation");

function UserClickButton2(elem) {
    input_box.value += "" + elem.innerText
}

function CalculateValue() {
    var input = input_box.value;
    var result = eval(input);
    input_box.value = result;
}

function ClearData() {
    input_box.value = "";
}