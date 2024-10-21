import { defineStore } from 'pinia'
import { KronosApi, type PantheonServiceName } from '@/services/kronos.api'
import type { ApiBase } from '@/services/api.base'

export const useBackend = defineStore('backend', {
  state: () => ({
    kronos: new KronosApi(),
  }),
  actions: {
    async getApi(name: PantheonServiceName): ApiBase {
      switch (name) {
        case 'Athena':
          return this.kronos.getAthenaApi()
        case 'Hermes':
          return this.kronos.getHermesApi()
      }
    },
  },
})
