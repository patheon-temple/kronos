import { ApiBase } from '@/services/api.base'
import type { IServiceDiscovery } from '@/services/models'

export class AthenaApi extends ApiBase {
  constructor(serviceDiscovery: IServiceDiscovery) {
    super(serviceDiscovery)
  }
}
