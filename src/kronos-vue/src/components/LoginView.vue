<script setup lang="ts">
import Password from 'primevue/password';

import InputText from 'primevue/inputtext';

import Button from 'primevue/button';

import { useBackend } from '@/stores/backend'

const backend = useBackend()

const loginClick =  async ()=>{
  const api = await backend.kronos.getHermesApi()
  await api.authenticateWithDeviceIdAsync('my device id is strong')
}

</script>

<template>
  <InputText type="text" v-model="value" />
  <Password v-model="value" :feedback="false" />
  <Button @click="loginClick" label="Login" />

</template>

<style scoped></style>
