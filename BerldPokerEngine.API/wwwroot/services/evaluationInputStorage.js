const evaluationInputStorageKey = "evaluationInput";

export function storeEvaluationInput(input) {
  localStorage.setItem(evaluationInputStorageKey, input);
}

export function getStoredEvaluationInput() {
  return localStorage.getItem(evaluationInputStorageKey);
}
