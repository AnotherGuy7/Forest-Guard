import pygame
import random
BLACK = (0,0,0)
WHITE = (255, 255, 255)

pygame.mixer.pre_init(frequency=44100, size=-16, channels=2, buffer=4096)
pygame.init()

standingFront = [pygame.image.load("Textures/Player_Standing_Front.png")]
standingBack = [pygame.image.load("Textures/Player_Standing_Back.png")]
standingLeft = [pygame.image.load("Textures/Player_Standing_Left.png")]
standingRight = [pygame.image.load("Textures/Player_Standing_Right.png")]

runningFront = [pygame.image.load("Textures/Player_Running_Front_1.png"),
                pygame.image.load("Textures/Player_Running_Front_2.png"),
                pygame.image.load("Textures/Player_Running_Front_3.png"),
                pygame.image.load("Textures/Player_Running_Front_2.png")]

runningBack = [pygame.image.load("Textures/Player_Running_Back_1.png"),
                pygame.image.load("Textures/Player_Running_Back_2.png"),
                pygame.image.load("Textures/Player_Running_Back_3.png"),
                pygame.image.load("Textures/Player_Running_Back_2.png")]

runningLeft = [pygame.image.load("Textures/Player_Running_Left_1.png"),
                pygame.image.load("Textures/Player_Running_Left_2.png"),
                pygame.image.load("Textures/Player_Running_Left_3.png"),
                pygame.image.load("Textures/Player_Running_Left_2.png")]

runningRight = [pygame.image.load("Textures/Player_Running_Right_1.png"),
                pygame.image.load("Textures/Player_Running_Right_2.png"),
                pygame.image.load("Textures/Player_Running_Right_3.png"),
                pygame.image.load("Textures/Player_Running_Right_2.png")]

swordTextures = [pygame.image.load("Textures/Sword_1.png"),
                pygame.image.load("Textures/Sword_2.png"),
                pygame.image.load("Textures/Sword_3.png"),
                pygame.image.load("Textures/Sword_4.png")]

stepSounds = [pygame.mixer.Sound("Sounds/Step1.wav"),
              pygame.mixer.Sound("Sounds/Step2.wav"),
              pygame.mixer.Sound("Sounds/Step3.wav"),
              pygame.mixer.Sound("Sounds/Step4.wav")]

swordSwingSound = pygame.mixer.Sound("Sounds/Sword_Swing.wav")

class Player(pygame.sprite.Sprite):
    
    def __init__(self):      #Initialization
        super().__init__()
        
        #Variables made in here are different in each instance (need the .self)
        self.health = 5
        self.frame = 0
        self.frameCounter = 0
        self.direction = "Front"
        self.moveSpeed = 7
        self.currentAnimationArray = standingFront
        self.swordRotation = 0
        self.swordSwingTimer = 0
        self.swordFrame = 0
        self.swordFrameCounter = 0
        self.swordAnimationArray = swordTextures
        self.swordOffsetX = 0
        self.swordOffsetY = 0
        self.immunityTimer = 0
        self.enemiesKilled = 0
        self.swingSound = swordSwingSound
        self.stepSounds = stepSounds
        self.walking = False
        
        self.image = pygame.Surface([60, 112])
        self.image.fill(BLACK)
        self.image.set_colorkey(BLACK)

        self.rect = self.image.get_rect()

        self.swordRect = self.image.get_rect()

    def Update(self):       #Update method
        if (self.immunityTimer > 0):
            self.immunityTimer -= 1
        if (self.swordSwingTimer > 0):
            self.swordSwingTimer -= 1

        self.walking = False
        keys = pygame.key.get_pressed() 
        if (keys[pygame.K_w] and self.rect.y > 10):
            self.walking = True
            self.direction = "Back"
            self.rect.y -= self.moveSpeed
            self.currentAnimationArray = runningBack
            
        if (keys[pygame.K_a] and self.rect.x > 0):
            self.walking = True
            self.direction = "Left"
            self.rect.x -= self.moveSpeed
            self.currentAnimationArray = runningLeft
            
        if (keys[pygame.K_s] and self.rect.y < 40 * 50):
            self.walking = True
            self.direction = "Front"
            self.rect.y += self.moveSpeed
            self.currentAnimationArray = runningFront
            
        if (keys[pygame.K_d] and self.rect.x < 44 * 50):
            self.walking = True
            self.direction = "Right"
            self.rect.x += self.moveSpeed
            self.currentAnimationArray = runningRight
            
        if (not keys[pygame.K_w] and not keys[pygame.K_a] and not keys[pygame.K_s] and not keys[pygame.K_d]):
            if (self.direction == "Front"):
                self.currentAnimationArray = standingFront
            if (self.direction == "Back"):
                self.currentAnimationArray = standingBack
            if (self.direction == "Left"):
                self.currentAnimationArray = standingLeft
            if (self.direction == "Right"):
                self.currentAnimationArray = standingRight

        if (pygame.mouse.get_pressed()[0] and self.swordSwingTimer <= 0):
            self.swordFrame = 0
            self.swordFrameCounter = 0
            self.swordSwingTimer = 14
            self.swingSound.play()
            if (self.direction == "Front"):
                self.swordOffsetX = 0
                self.swordOffsetY = 0
                self.swordRotation = 0
            if (self.direction == "Back"):
                self.swordOffsetX = 0
                self.swordOffsetY = -40
                self.swordRotation = 180
            if (self.direction == "Left"):
                self.swordOffsetX = -60
                self.swordOffsetY = 0
                self.swordRotation = 270
            if (self.direction == "Right"):
                self.swordOffsetX = -20
                self.swordOffsetY = 0
                self.swordRotation = 90
        
        self.AnimatePlayer()

    def AnimatePlayer(self):
        self.frameCounter += 1
        if (self.frameCounter > 5):
            self.frame += 1
            self.frameCounter = 0
            
        if (self.frame >= len(self.currentAnimationArray)):
            self.frame = 0

        if (self.walking and self.frameCounter == 0 and(self.frame == 0 or self.frame == 2)):
            self.stepSounds[random.randint(0, len(self.stepSounds) - 1)].play()

        if (self.swordSwingTimer > 0):
            self.swordFrameCounter += 1

            if (self.swordFrameCounter >= 4):
                self.swordFrame += 1
                self.swordFrameCounter = 0

            if (self.swordFrame >= len(self.swordAnimationArray)):
                self.swordFrame = 0
            
        
    def Draw(self, screen, cameraPosition):     #Draw method
        screenWidth = screen.get_width()
        screenHeight = screen.get_height()
        screen.blit(self.currentAnimationArray[self.frame], (self.rect.x - cameraPosition[0] + (screenWidth / 2), self.rect.y - cameraPosition[1] + (screenHeight / 2)))     #The camera is right on the player

        if (self.swordSwingTimer > 0):
            texture = pygame.transform.rotate(self.swordAnimationArray[self.swordFrame], self.swordRotation)        #Rotating the sword
            screen.blit(texture, (self.rect.x - cameraPosition[0] + (screenWidth / 2) + self.swordOffsetX, self.rect.y - cameraPosition[1] + (screenHeight / 2) + self.swordOffsetY))
            self.swordRect = texture.get_rect()
            self.swordRect.x = self.rect.x + self.swordOffsetX
            self.swordRect.y = self.rect.y + self.swordOffsetY
