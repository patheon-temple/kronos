import { Api } from '@/api/Api'

export function createApiClient(accessToken?: string) {
  return new Api({
    baseUrl: 'https://localhost:7115',
    securityWorker: () => ({
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }),
  })
}
