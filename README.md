# Radar Trainer

Radar Trainer is a browser-based radar training simulator. It is not a navigation tool.

## GitHub Pages deployment

The GitHub Actions workflow deploys pushes to `Main` and can also be started manually from the Actions tab. In the repository's **Settings > Pages**, set **Build and deployment > Source** to **GitHub Actions**. Push the workflow and application changes to GitHub, then check the Actions tab for the deployment result. The deployment job provides the published URL; for this repository it is expected to be `https://johnpmaguire.github.io/RadarTrainer/`.

The workflow sets the Blazor base path from the Pages configuration before publishing, so static assets and the service worker work under a repository subpath. It also copies the published app shell to `404.html` so direct links and refreshes on client-side routes load the app. GitHub Pages still returns HTTP 404 for those direct route requests even when the app loads; unknown routes display the app's Not Found page.

Changes to the offline-enabled published app may not appear in an already open tab immediately. Close all tabs using the app and reopen it to allow the updated service worker to take over.
