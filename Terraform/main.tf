terraform {
 required_providers {
 docker = {
 source = "kreuzwerker/docker"
 }
 }

}
provider "docker" {}
# Image Ubuntu
resource "docker_image" "ubuntu" {
 name = "ubuntu:latest"
}
# Container (simule une VM)
resource "docker_container" "server" {
 name = "devops-server"
 image = docker_image.ubuntu.name
 tty = true
 command = ["sleep", "3600"]
 ports {
 internal = 80
 external = 6000
 }



}




resource "docker_image" "api" {
 name = "moon2003/incidents-api:1.0"
}
resource "docker_container" "api_container" {
 name = "incidents-api-container"
 image = docker_image.api.name
 ports {
 internal = 80
 external = 6001
 }
 env = [
 "ASPNETCORE_ENVIRONMENT=Development"
 ]
} 

