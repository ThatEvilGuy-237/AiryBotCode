#!/bin/bash
set -e

# === Config ===
PROJECT_ID="melodic-map-460716-a1"
REGION="europe-west1"
REPOSITORY="airy-guardian"
IMAGE_NAME="airybot"
TAG="latest"
CONTAINER_NAME="airybot-container"

# Install Docker
echo "Installing Docker..."
sudo apt update
sudo apt install -y ca-certificates curl gnupg lsb-release

sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

sudo usermod -aG docker $USER
echo "Docker installed. You may need to logout/login to run docker without sudo."

# Google Cloud login
echo "Please login to Google Cloud..."
gcloud auth login

# Configure Docker auth for Artifact Registry
echo "Configuring Docker authentication for Artifact Registry..."
gcloud auth configure-docker ${REGION}-docker.pkg.dev

# Pull and run container
FULL_IMAGE="${REGION}-docker.pkg.dev/${PROJECT_ID}/${REPOSITORY}/${IMAGE_NAME}:${TAG}"

echo "Pulling container image $FULL_IMAGE"
docker pull "$FULL_IMAGE"

echo "Running container $CONTAINER_NAME"
docker run -d --name "$CONTAINER_NAME" "$FULL_IMAGE"

echo "Done. Container should now be running."
sleep 5

echo "Showing logs for container $CONTAINER_NAME:"
docker logs "$CONTAINER_NAME"

echo ""
echo "To check the logs later, run:"
echo "  docker logs $CONTAINER_NAME"