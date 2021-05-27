import pygame
BLACK = (0,0,0)
WHITE = (255, 255, 255)

texture = pygame.image.load("Textures/Tiles/Tree.png")
    
class Tree(pygame.sprite.Sprite):

    def __init__(self, posX, posY):
        super().__init__()

        self.width = 50
        self.height = 100
        self.texture = texture
        
        self.rect = self.texture.get_rect()
        self.rect.x = posX
        self.rect.y = posY
        
    def Draw(self, screen, cameraPosition):     #Draw method
        screen.blit(self.texture, (self.rect.x - cameraPosition[0], self.rect.y - cameraPosition[1]))
