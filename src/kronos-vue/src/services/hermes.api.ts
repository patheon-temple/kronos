import { ApiBase } from '@/services/api.base'
import type { IServiceDiscovery } from '@/services/models'

export class HermesApi extends ApiBase {
  constructor(serviceDiscovery: IServiceDiscovery) {
    super(serviceDiscovery)
  }

  async authenticateWithDeviceIdAsync(deviceId: string): Promise<void> {
    return new Promise(resolve => setTimeout(() => resolve(), 1000))
  }
}
