function getValue(id) {
  return document.getElementById(id).value;
}

function setValue(id, value) {
  document.getElementById(id).value = value;
}

function displayMessage(msg) {
  document.getElementById("message").innerHTML = msg;
}

function displayError(error) {
  document.getElementById("error").innerHTML = JSON.stringify(error);
}

function handleAjaxError(error) {
  displayError(error);

  switch (error.status) {
    case 404:
      console.log(error.responseText);
      break;
    case 500:
      console.log(error.responseText);
      break;
    default:
      console.log(error);
      break;
  }
}