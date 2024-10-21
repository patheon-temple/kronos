import type { ServiceBase } from '@/services/service.base'

export class HermesService extends ServiceBase {
  constructor() {
    super('Hermes')
  }

  async authenticateWithDeviceIdAsync() : Promise<void> {

  }
}
