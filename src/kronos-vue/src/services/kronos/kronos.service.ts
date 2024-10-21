import { assertSuccessStatusCode, throwErrorStatusCode } from '@/services/helpers'

export type PantheonServiceName = 'Athena' | 'Hermes';

export interface IServiceDiscovery {
  url: string
  description: string
}

export class KronosService {
  private readonly _baseUrl: string = import.meta.env.VITE_KRONOS_API_BASE_URL
  private _services: Record<PantheonServiceName, IServiceDiscovery> | undefined

  constructor() {

  }

  async loadServiceDiscovery(): Promise<Record<PantheonServiceName, IServiceDiscovery> | undefined> {
    if(this._services) return this._services
    const response = await fetch(`${this._baseUrl}/kronos/api/v1`)
    if (!assertSuccessStatusCode(response.status)) await throwErrorStatusCode(response)

    this._services = await response.json()

    return this._services;
  }
}
