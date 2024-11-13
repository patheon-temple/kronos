import { defineStore } from 'pinia'
import type { AuthenticationSuccessfulResponse } from '@/api/Api'

export const useUserStore = defineStore('user-store', {
  state: () => {
    const jsonRaw = window.localStorage.getItem('authenticationData')
    if (jsonRaw) {
      return {
        isAuthenticated: true,
        authenticationData: JSON.parse(jsonRaw),
      }
    }
    return {
      isAuthenticated: false,
      authenticationData: {} as AuthenticationSuccessfulResponse,
    }
  },
  actions: {
    storeAuthenticationData(data: AuthenticationSuccessfulResponse): void {
      this.authenticationData = data
      this.isAuthenticated = data != undefined
      window.localStorage.setItem('authenticationData', JSON.stringify(data))
    },
    invalidateAuthenticationData(): void {
      this.authenticationData = undefined
      this.isAuthenticated = false
      window.localStorage.removeItem('authenticationData')
    },
  },
})
