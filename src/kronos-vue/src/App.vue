<script setup lang="ts">
import Menubar from 'primevue/menubar'
import { ref } from 'vue'
import type { MenuItem } from 'primevue/menuitem'

import Toast from 'primevue/toast';
import LoginView from '@/components/LoginView.vue'

const menuItems = ref<MenuItem[]>([
  {
    label: 'Home',
    route: '/',
    icon: 'pi pi-home',
  },
  {
    label: 'Users',
    route: '/users',
    icon: 'pi pi-user',
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
    <RouterView />
    <LoginView />
  </main>
  <Toast />
</template>

<style scoped></style>
