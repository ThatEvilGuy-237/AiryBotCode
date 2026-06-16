<script setup lang="ts">
// PROOF that the Hive's shared component library (@hive/ui), vendored into this panel
// (see vendor/hive-ui), builds and renders here — the foundation for rebuilding the
// panel on the shared UI (roadmap item E). Existing panel pages are untouched.
import { Button, Badge, Card, PageHeader } from '@hive/ui'
import '@hive/ui/theme.css'
import '@hive/ui/components.css'
</script>

<template>
  <div class="hive-ui-proof">
    <PageHeader title="@hive/ui — vendored proof" subtitle="Shared Hive components rendering inside the Airy panel" />

    <Card>
      <p>
        These controls come from <strong>@hive/ui</strong> (The Hive's Aether-Sight library),
        vendored as a built package. <Badge>vendored</Badge>
      </p>
      <div style="display:flex; gap:.5rem; margin-top:.75rem; flex-wrap:wrap">
        <Button>Primary action</Button>
        <Badge>new</Badge>
      </div>
    </Card>
  </div>
</template>

<style scoped>
.hive-ui-proof { padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem; }
</style>
