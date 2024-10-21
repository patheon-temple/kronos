import {
  assertSuccessStatusCode,
  throwErrorStatusCode,
} from '@/services/helpers'

import { HermesApi } from '@/services/hermes.api'
import { AthenaApi } from '@/services/athena.api'
import type { IServiceDiscovery } from '@/services/models'

export type PantheonServiceName = 'Athena' | 'Hermes'

export class KronosApi {
  private readonly _baseUrl: string = import.meta.env.VITE_KRONOS_API_BASE_URL

  private _hermesApi?: HermesApi
  private _services?: Record<PantheonServiceName, IServiceDiscovery>

  constructor() {
    if (!this._baseUrl) throw 'VITE_KRONOS_API_BASE_URL=undefined'
  }

  async getHermesApi(): Promise<HermesApi> {
    if (this._hermesApi) return this._hermesApi

    await this.loadServiceDiscovery()
    this._hermesApi = new HermesApi(this._services!.Hermes)
    return this._hermesApi
  }


  async getAthenaApi(): Promise<AthenaApi> {
    if (this._hermesApi) return this._hermesApi

    await this.loadServiceDiscovery()
    this._hermesApi = new AthenaApi(this._services!.Hermes)
    return this._hermesApi
  }

  private async loadServiceDiscovery(): Promise<void> {
    if (this._services) return this._services
    try {
      const response = await fetch(`${this._baseUrl}/kronos/api/v1`)
      if (!assertSuccessStatusCode(response.status))
        await throwErrorStatusCode(response)

      this._services = await response.json()

      return this._services
    } catch (error) {
      console.error(error)
      return undefined
    }
  }
}
