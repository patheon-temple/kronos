<script setup lang="ts">
import Panel from 'primevue/panel'
import type { PantheonServiceName } from '@/services/kronos.api'
import { onMounted, ref } from 'vue'
import { useKronos } from '@/stores/kronos'
import { useApi } from '@/services/service.factory'

const props = defineProps<{
  name: PantheonServiceName
}>()
const kronos = useKronos()
const isHealthy = ref<boolean>(false)
const healthcheckIntervalHandle = ref<number | undefined>()
const lastCheck = ref<Date | undefined>(undefined)

onMounted(() => {
  useApi(kronos.api, props.name).then(api => {
    healthcheckIntervalHandle.value = setInterval(async () => {
      isHealthy.value = await api.healthcheck()
      lastCheck.value = new Date()
    }, Math.max(1000, Math.random() * 1000 + 500))
  })
})
</script>

<template>
  <Panel :header="name" toggleable collapsed>
    <template #icons>
      <i class="pi pi-heart-fill mx-1.5" v-if="isHealthy"></i>
      <i class="pi pi-heart m-0.5" v-else></i>
    </template>
    Last healthcheck: {{ lastCheck?.toISOString() }}
  </Panel>
</template>

<style scoped></style>
