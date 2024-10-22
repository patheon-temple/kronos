<script setup lang="ts">
import Menubar from 'primevue/menubar'
import HealthcheckWidget from '@/components/HealthcheckWidget.vue'
import { ref } from 'vue'
import type { MenuItem } from 'primevue/menuitem'

import Toast from 'primevue/toast';
import { useUserStore } from '@/stores/user.store'
import LoginView from '@/components/LoginView.vue'


const userStore = useUserStore()
const menuItems = ref<MenuItem[]>([
  {
    label: 'Home',
    route: '/',
    icon: 'pi pi-home',
  }
])
</script>

<template>
  <main  class="mx-1.5 my-1">
    <nav>
      <Menubar :model="menuItems" >
        <template #item="{ item, props, hasSubmenu }">
          <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
            <a v-ripple :href="href" v-bind="props.action" @click="navigate">
              <span :class="item.icon" />
              <span>{{ item.label }}</span>
            </a>
          </router-link>
          <a v-else v-ripple :href="item.url" :target="item.target" v-bind="props.action">
            <span :class="item.icon" />
            <span>{{ item.label }}</span>
            <span v-if="hasSubmenu" class="pi pi-fw pi-angle-down" />
          </a>
        </template>
      </Menubar>
    </nav>
    <div class="flex flex-row gap-1 align-items-center">
      <strong class="my-auto mx-1">Service status</strong>
      <HealthcheckWidget name="Hermes" />
      <HealthcheckWidget name="Athena" />
    </div>
    <RouterView />
    <LoginView />
  </main>
  <Toast />
</template>

<style scoped></style>
