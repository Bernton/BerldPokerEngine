const baseUrl = window.location.origin;
const evaluationUrl = baseUrl + "/evaluate";

export async function evaluate(evaluationInput) {
  const searchParams = { input: evaluationInput };
  const request = evaluationUrl + getQueryString(searchParams);
  const response = await fetch(request);
  return response;
}

function getQueryString(searchParams) {
  return "?" + new URLSearchParams(searchParams);
}
