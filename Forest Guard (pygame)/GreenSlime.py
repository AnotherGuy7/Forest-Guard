import pygame
BLACK = (0,0,0)
WHITE = (255, 255, 255)

frames = [pygame.image.load("Textures/Slime_1.png"),
        pygame.image.load("Textures/Slime_2.png")]

class GreenSlimeEnemy(pygame.sprite.Sprite):
    
    def __init__(self):
        super().__init__()
        
        self.frame = 0
        self.frameCounter = 1
        self.health = 2
        self.immunityTimer = 0
        self.enemy = True
        self.direction = 1
        self.moveSpeed = 1.2
        
        self.image = pygame.Surface([70, 70])
        self.image.fill(BLACK)
        self.image.set_colorkey(BLACK)

        self.image = frames[0]

        self.rect = self.image.get_rect()

    def UpdateAI(self, player):
        if (self.immunityTimer > 0):
            self.immunityTimer -= 1
        
        if (self.rect.x > player.rect.x):
            self.direction = -1
        else:
            self.direction = 2      #This is because for some unknown reason, adding 1.2 
            
        if (self.rect.y > player.rect.y):
            self.rect.y -= self.moveSpeed
        else:
            self.rect.y += self.moveSpeed * 2

        self.rect.x += float(self.moveSpeed * self.direction)

        self.AnimateSlime()

    def AnimateSlime(self):
        self.frameCounter += 1
        if (self.frameCounter >= 5):
            self.frame += 1
            self.frameCounter = 0
            if (self.frame >= 2):
                self.frame = 0

    def Hurt(self):
        self.health -= 1
        self.immunityTimer += 15

    def Draw(self, screen, cameraPosition):
        screenWidth = screen.get_width()
        screenHeight = screen.get_height()

        if (self.direction == -1):
            texture = pygame.transform.flip(frames[self.frame], True, False)
        else:
            texture = frames[self.frame]
        screen.blit(texture, (self.rect.x - cameraPosition[0] + (screenWidth / 2), self.rect.y - cameraPosition[1] + (screenHeight / 2)))
