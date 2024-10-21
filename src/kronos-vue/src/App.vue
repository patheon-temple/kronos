<script setup lang="ts">
import { onMounted } from 'vue'
import { KronosService } from '@/services/kronos/kronos.service'

onMounted(async ()=>{
  const kronos = new KronosService();
  const services = await kronos.loadServiceDiscovery()

  console.log(services?.Athena.description)
  console.log(services?.Hermes.description)
})
</script>

<template>
  <p>
    <strong>Current route path:</strong> {{ $route.fullPath }}
  </p>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/Login">Go to About</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>

<style scoped>

</style>
