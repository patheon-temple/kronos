import { Api } from '@/api/Api'

export function createApiClient(accessToken?: string) {
  return new Api({
    baseUrl: 'https://localhost:7115',
    baseApiParams: {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  })
}
