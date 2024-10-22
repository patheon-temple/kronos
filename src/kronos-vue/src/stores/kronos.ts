import { defineStore } from 'pinia'
import { KronosApi } from '@/services/kronos.api'

export const useKronos = defineStore('kronos', {
  state: () => ({
    api: new KronosApi(),
  })
})
