import type { KronosApi, PantheonServiceName } from '@/services/kronos.api'
import { HermesApi } from '@/services/hermes.api'
import { AthenaApi } from '@/services/athena.api'
import { ApiBase } from '@/services/api.base'

export async function useHermesApi(kronos: KronosApi) {
  const discovery = await kronos.loadServiceDiscovery()
  return new HermesApi(discovery.Hermes)
}

export async function useAthenaApi(kronos: KronosApi) {
  const discovery = await kronos.loadServiceDiscovery()
  return new AthenaApi(discovery.Athena)
}

export async function useApi(kronos: KronosApi, api: PantheonServiceName) {
  const discovery = await kronos.loadServiceDiscovery()
  return new ApiBase(discovery[api])
}
