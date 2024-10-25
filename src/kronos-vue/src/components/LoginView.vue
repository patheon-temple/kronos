<script setup lang="ts">
import Dialog from 'primevue/dialog'

import Password from 'primevue/password'

import InputText from 'primevue/inputtext'

import Button from 'primevue/button'

import { useKronos } from '@/stores/kronos'
import { ref } from 'vue'
import { useHermesApi } from '@/services/service.factory'
import { useToast } from 'primevue/usetoast'
import { useUserStore } from '@/stores/user.store'

const kronos = useKronos()
const toast = useToast()
const userStore = useUserStore()

const isProcessingLogin = ref(false)
const loginClick = async () => {
  isProcessingLogin.value = true
  try {
    const hermes = await useHermesApi(kronos.api)

    await hermes.authenticateWithDeviceIdAsync('my device id is strong')
    userStore.isAuthenticated = true

    toast.add({
      closable: false,
      severity: 'success',
      life: 1000,
      summary: 'Login successful',
    })
  } catch (error) {
    userStore.isAuthenticated = false
    toast.add({
      closable: false,
      severity: 'error',
      life: 1000,
      summary: error.message,
    })
  } finally {
    isProcessingLogin.value = false
  }
}
</script>

<template>
  <Dialog
    v-bind:visible="!userStore.isAuthenticated"
    :closable="false"
    :draggable="false"
    modal
    header="Login"
    :style="{ width: '25rem' }"
  >
    <div class="flex items-center gap-4 mb-4">
      <label for="username" class="font-semibold w-24">Username</label>
      <InputText id="username" class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex items-center gap-4 mb-8">
      <label for="password" class="font-semibold w-24">Password</label>
      <Password id="password" class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex justify-end gap-2">
      <Button
        :disabled="isProcessingLogin"
        type="button"
        label="Login"
        @click="loginClick"
      ></Button>
    </div>
  </Dialog>
</template>

<style scoped></style>
