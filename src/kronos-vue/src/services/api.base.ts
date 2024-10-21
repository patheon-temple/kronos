import { assertSuccessStatusCode } from '@/services/helpers'
import type { IServiceDiscovery } from '@/services/models'

export abstract class ApiBase {
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
      const response = await fetch(this._serviceDiscovery.healthcheckUrl)
      return assertSuccessStatusCode(response.status)
    } catch (error) {
      console.error(error)
      return false
    }
  }
}
