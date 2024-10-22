import { defineStore } from 'pinia'

export const useUserStore = defineStore('user-store', {
  state: () => ({
    isAuthenticated: false,
  })
})
