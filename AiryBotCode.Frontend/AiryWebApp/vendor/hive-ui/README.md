# Vendored @hive/ui

Built artifacts of The Hive's shared component library (`Aether-Sight/dist`), vendored
here because the panel builds in its own Docker context (no cross-repo path-link works).

**Re-vendor after @hive/ui changes** (run in ~/The-Hive/Aether-Sight, then copy):
    npm run build        # produces dist/  (needs the Hive API for generate:api)
    cp dist/* <panel>/vendor/hive-ui/dist/

`vue` and `vue-router` are peer deps (externalized in the lib build) — provided by the panel.
