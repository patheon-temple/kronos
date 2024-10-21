export const successStatusCode = (code: number) => {
  return code >= 200 && code < 400
}

export async function throwErrorStatusCode (response: Response)  {
  const text = await response.text()
  throw new Error(`request failed with status code: ${response.status} -  ${text}`)
}
