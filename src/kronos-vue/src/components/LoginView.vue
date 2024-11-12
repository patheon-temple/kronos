<script setup lang="ts">
import Dialog from 'primevue/dialog'

import Password from 'primevue/password'

import InputText from 'primevue/inputtext'

import Button from 'primevue/button'

import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useUserStore } from '@/stores/user.store'
import { Api, CredentialsType } from '@/api/Api'
import { createApiClient } from '@/factories/api.factory'

const toast = useToast()
const userStore = useUserStore()
const usernameInput = ref<string | undefined>(undefined)
const passwordInput = ref<string | undefined>(undefined)

const isProcessingLogin = ref(false)
const loginClick = async () => {
  isProcessingLogin.value = true
  try {
    const apiClient = createApiClient()
    const response = await apiClient.hermes.authenticate({
      credentialsType: CredentialsType.Password,
      password: passwordInput.value,
      username: usernameInput.value,
    })

    if (!response.ok) {
      toast.add({
        summary: `${response.status}: ${response.statusText}`,
      })
      return
    }
    userStore.storeAuthenticationData(response.data)
    toast.add({
      severity: 'success',
      summary: 'Successfully authenticated',
    })
  } catch (error) {
    console.error(error)
    toast.add({
      summary:  'Failed to authenticate',
      severity: 'error',
    })
    return
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
      <InputText
        id="username"
        v-model="usernameInput"
        class="flex-auto"
        autocomplete="off"
      />
    </div>
    <div class="flex items-center gap-4 mb-8">
      <label for="password" class="font-semibold w-24">Password</label>
      <Password
        id="password"
        v-model="passwordInput"
        class="flex-auto"
        autocomplete="off"
      />
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
