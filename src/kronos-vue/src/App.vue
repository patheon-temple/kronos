<script setup lang="ts">


import HealthcheckWidget from '@/components/HealthcheckWidget.vue'
</script>

<template>
  <p>
    <strong>Current route path:</strong> {{ $route.fullPath }}
  </p>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/login">Go to About</RouterLink>
  </nav>
  <main>
<!--    <HealthcheckWidget name="Athena"/>-->
    <HealthcheckWidget name="Hermes"/>
    <HealthcheckWidget name="Athena"/>
    <RouterView />
  </main>
</template>

<style scoped>

</style>
