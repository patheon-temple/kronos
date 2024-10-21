import type { IServiceDiscovery } from '@/services/kronos/kronos.service'
import { assertSuccessStatusCode, throwErrorStatusCode } from '@/services/helpers'

export abstract class ServiceBase {
  private readonly _serviceDiscovery: IServiceDiscovery

  protected constructor(serviceDiscovery: IServiceDiscovery) {
    this._serviceDiscovery = serviceDiscovery
  }

  protected httpGet() {

  }

  private composeUrl(path: string): string {
    return `${this._serviceDiscovery.url}${path}`
  }

  public async healthcheck(): Promise<boolean> {
    try {
      const response = await fetch(this.composeUrl('/healthz'))
      return assertSuccessStatusCode(response.status)
    } catch (error) {
      console.error(error)
      return false
    }
  }
}
