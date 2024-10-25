import './assets/main.css'
import Ripple from 'primevue/ripple';
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/DashboardView.vue'
import LoginView from '@/components/LoginView.vue'
import ToastService from 'primevue/toastservice';

const routes = [
  { path: '/', component: DashboardView },
  { path: '/login', component: LoginView }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

const app = createApp(App)

app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      cssLayer: {
        name: 'primevue',
        order: 'tailwind-base, primevue, tailwind-utilities'
      }
    }
  },
  ripple: true
})
  .use(router)
  .use(ToastService)
  .use(createPinia())
app.directive('ripple', Ripple);

app.mount('#app')
