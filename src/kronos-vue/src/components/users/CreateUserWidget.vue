<script setup lang="ts">
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import { computed, ref } from 'vue'
import { useUserStore } from '@/stores/user.store'
import { createApiClient } from '@/factories/api.factory'

const usernameInput = ref<string | undefined>(undefined)
const passwordInput = ref<string | undefined>(undefined)
const deviceIdInput = ref<string | undefined>(undefined)
const isProcessing = ref<boolean>(false)
const isPasswordValid = computed(()=>{
  return /^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])(?=.*[a-z])[A-Za-z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{7,128}$/.test(passwordInput.value || '')
})
const userStore = useUserStore()
const createUserAction = async () => {

    const api =createApiClient(userStore.authenticationData.accessToken)
    await api.athena.apiAdminV1AccountUserCreate({
      deviceId: deviceIdInput.value,
      password: passwordInput.value,
      username: usernameInput.value,
    })
}

defineProps<{
  isVisible: boolean
}>()
</script>

<template>
  <Dialog
    v-bind:visible="isVisible"
    :closable="false"
    :draggable="false"
    modal
    header="Create user"
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
    <div class="flex items-center gap-4 mb-4">
      <label for="username" class="font-semibold w-24">Password</label>
      <InputText
        id="password"
        v-model="passwordInput"
        :invalid="!isPasswordValid"
        class="flex-auto"
        autocomplete="off"
      />
    </div>
    <div class="flex items-center gap-4 mb-4">
      <label for="username" class="font-semibold w-24">Device Id</label>
      <InputText
        id="deviceId"
        v-model="deviceIdInput"
        class="flex-auto"
        autocomplete="off"
      />
    </div>
    <div class="flex justify-end gap-2">
      <Button
        :disabled="isProcessing"
        type="button"
        label="Create User"
        @click="createUserAction"
      ></Button>
    </div>
  </Dialog>
</template>

<style scoped></style>
