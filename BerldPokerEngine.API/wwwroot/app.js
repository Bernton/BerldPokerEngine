const evaluationInputStorageKey = "evaluationInput";

const evaluationInputElement = document.getElementById("evaluation-input");
const evaluateElement = document.getElementById("evaluate");
const errorMessageElement = document.getElementById("error-message");
const evaluationLoaderElement = document.getElementById("evaluation-loader");
const evaluationOuputElement = document.getElementById("evaluation-output");

const baseUrl = window.location.href;
const evaluationUrl = baseUrl + "evaluate";

const defaultInititalEvaluationInput = "XxXxXxXxXx XxXx";
const initialEvaluationInput =
  getStoredEvaluationInput() ?? defaultInititalEvaluationInput;

evaluationInputElement.value = initialEvaluationInput;
evaluateElement.addEventListener("click", () => wrapLoader(evaluate));

async function wrapLoader(action) {
  evaluationLoaderElement.style.display = "block";
  await action();
  evaluationLoaderElement.style.display = "none";
}

async function evaluate() {
  const evaluationInput = evaluationInputElement.value.trim();
  const searchParams = { input: evaluationInput };
  const request = evaluationUrl + getQueryString(searchParams);
  const response = await fetch(request);
  const data = await response.json();

  if (response.ok) {
    updateHandler(evaluationInput, data);
  } else {
    errorHandler(data);
  }
}

function getQueryString(searchParams) {
  return "?" + new URLSearchParams(searchParams);
}

function updateHandler(evaluationInput, data) {
  storeEvaluationInput(evaluationInput);
  const outputText = JSON.stringify(data, null, 2);
  evaluationOuputElement.innerText = outputText;
}

function errorHandler(data) {
  errorMessageElement.innerText = data;
}

function storeEvaluationInput(input) {
  localStorage.setItem(evaluationInputStorageKey, input);
}

function getStoredEvaluationInput() {
  return localStorage.getItem(evaluationInputStorageKey);
}
