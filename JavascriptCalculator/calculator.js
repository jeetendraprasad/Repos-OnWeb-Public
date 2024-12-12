var input_box = document.getElementById("calculation");

function UserClickButton(input) {
    input_box.value += "" + input;
}

function CalculateValue() {
    var input = input_box.value;
    var result = eval(input);
    input_box.value = result;
}

function ClearData() {
    input_box.value = "";
}