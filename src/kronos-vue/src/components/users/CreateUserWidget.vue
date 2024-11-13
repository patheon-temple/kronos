<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import { useUserStore } from '@/stores/user.store'
import { createApiClient } from '@/factories/api.factory'
import { Form } from '@primevue/forms'
import { useToast } from 'primevue/usetoast'
import { z } from 'zod';
import * as zRes from '@primevue/forms/resolvers/zod';

const toast = useToast()
const userStore = useUserStore()

const initialValues = reactive({
  username: '',
  deviceId: '',
  password: ''
})
const resolver = ref(zRes.zodResolver(
  z.object({
    username: z.string().min(1, { message: 'Username is required' }),
    password: z.string().refine((value) => /[a-z]/.test(value), {
      message: 'Password must be between 7 and 128 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character from the following: !@#$%^&*()_+-=[]{};\':"\\\\|,.<>/?.'
    }),
  })
));
const onFormSubmit = async (e) => {
  if (e.valid) {
    try {
      await createApiClient(userStore.authenticationData.accessToken).athena.apiAdminV1AccountUserCreate(values)
      toast.add({
        severity: 'success',
        summary: 'User account created'
      })
    } catch (e) {
      console.error(e)
      toast.add({
        severity: 'error',
        summary: 'Failed to create user'
      })
    }

  }
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
    <Form v-slot="$form" :initialValues :resolver @submit="onFormSubmit" class="flex flex-col gap-4 w-full sm:w-56">
      <div class="flex flex-col gap-1">
        <InputText name="username" type="text" placeholder="Username" fluid />
        <Message v-if="$form.username?.invalid" severity="error" size="small" variant="simple">
          {{ $form.username.error?.message }}
        </Message>
      </div>
      <div class="flex flex-col gap-1">
        <InputText name="password" type="text" placeholder="Password" fluid />
        <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
          {{ $form.password.error?.message }}
        </Message>
      </div>
      <div class="flex flex-col gap-1">
        <InputText name="deviceId" type="text" placeholder="Device ID" fluid />
        <Message v-if="$form.deviceId?.invalid" severity="error" size="small" variant="simple">
          {{ $form.deviceId.error?.message }}
        </Message>
      </div>
      <Button type="submit" severity="secondary" label="Submit" />
    </Form>
  </Dialog>
</template>

<style scoped></style>
