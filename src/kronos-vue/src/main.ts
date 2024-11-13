import './assets/main.css'
import Ripple from 'primevue/ripple'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/DashboardView.vue'
import ToastService from 'primevue/toastservice'
import UsersTableView from '@/components/users/UsersTableView.vue'
import { useUserStore } from '@/stores/user.store'

const { fetch: originalFetch } = window;

window.fetch = async (...args) => {
  const [resource, config ] = args;
  // request interceptor here
  const response = await originalFetch(resource, config);

  if(response.status === 401){
    const userStore = useUserStore()
    userStore.invalidateAuthenticationData()
  }

  // response interceptor here
  return response;
};

const routes = [
  { path: '/', component: DashboardView },
  { path: '/users', component: UsersTableView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

const app = createApp(App)

app
  .use(PrimeVue, {
    theme: {
      preset: Aura,
      options: {
        cssLayer: {
          name: 'primevue',
          order: 'tailwind-base, primevue, tailwind-utilities',
        },
      },
    },
    ripple: true,
  })
  .use(router)
  .use(ToastService)
  .use(createPinia())
app.directive('ripple', Ripple)

app.mount('#app')
