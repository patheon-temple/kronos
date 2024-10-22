<script setup lang="ts">
import Panel from 'primevue/panel'
import type { PantheonServiceName } from '@/services/kronos.api'
import { onMounted, ref } from 'vue'
import { useBackend } from '@/stores/backend'

const props = defineProps<{
  name: PantheonServiceName
}>()
const backend = useBackend()
const isHealthy = ref<boolean>(false)
const healthcheckIntervalHandle = ref<number | undefined>()
const lastCheck = ref<Date | undefined>(undefined)

onMounted(() => {
  backend.getApi(props.name).then(api => {
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
      <i class="pi pi-heart-fill" v-if="isHealthy"></i>
      <i class="pi pi-heart" v-else></i>
    </template>
    {{ lastCheck?.toISOString() }}
  </Panel>
</template>

<style scoped></style>
