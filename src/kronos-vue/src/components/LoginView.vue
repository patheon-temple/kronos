<script setup lang="ts">
import { useBackend } from '@/stores/backend'

const backend = useBackend()

const loginClick =  async ()=>{
  const api = await backend.kronos.getHermesApi()
  await api.authenticateWithDeviceIdAsync('my device id is strong')
}

</script>

<template>
  <button @click="loginClick">Login</button>
</template>

<style scoped></style>
