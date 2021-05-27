import random
import pygame
from PlayerClass import Player
from GreenSlime import GreenSlimeEnemy
from OrangeSlime import OrangeSlimeEnemy
from TreeClass import Tree

BLACK = (0, 0, 0, 0)

pygame.init()
size = (1200, 800)
screen = pygame.display.set_mode(size)
pygame.display.set_caption("Forest Guard")

#Making groups for organization
drawsList = pygame.sprite.Group()           #A list of what gets drawn    
entitiesList = pygame.sprite.Group()        #A list of what gets updated

player = Player()
player.rect.x = size[0] / 2
player.rect.y = size[1] / 2
player.health = 5
drawsList.add(player)
entitiesList.add(player)

gameState = 0       #Variables for the game's state
GAMESTATE_TITLE = 0
GAMESTATE_PLAYING = 1
GAMESTATE_GAMEOVER = 2

MUSIC_TITLE = 0
MUSIC_GAME = 1

mapWidth = 71       #The maps width in pixels is mapWidth * 50
mapHeight = 60
mapTilesList = []

clock = pygame.time.Clock()

gameRunning = True
cameraPosition = [player.rect.x, player.rect.y]
showingRules = False

grassTile1 = pygame.image.load("Textures/Tiles/GrassTile_1.png")
grassTile2 = pygame.image.load("Textures/Tiles/GrassTile_2.png")
titleScreenBackground = pygame.image.load("Textures/Backgrounds/Background.png")
gameOverBackground = pygame.image.load("Textures/Backgrounds/GameOverBackground.png")
#titleScreenMusic = pygame.mixer.music.load("Sounds/Music/TitleSong.mp3")       #Apparently different music isn't easy to change with just the one .music variable. Weird python stuff.
#mainGameMusic = pygame.mixer.music.load("Sounds/Music/Main_Chill_Action.mp3")
slimeHitSound = pygame.mixer.Sound("Sounds/Slime_Hit.wav")

currentRoundNumber = 0

pygame.mixer.pre_init(frequency=44100, size=-16, channels=2, buffer=4096)
pygame.init()

def InitializeGame():       #A method for restarting the game and related variables
    player.enemiesKilled = 0
    currentRoundNumber = 0
    player.health = 5
    player.rect.x = size[0] / 2
    player.rect.y = size[1] / 2

    for draw in drawsList:
        if (draw != player):
            drawsList.remove(draw)

    for entity in entitiesList:
        if (entity != player):
            entitiesList.remove(entity)
    
    mapTilesList.clear()
    
    GenerateMap()
    UpdateScreen()
    StartNewRound()
    PlayMusic(MUSIC_GAME)

def StartNewRound():
    player.health += 1
    for i in range(0, 3 * (int((currentRoundNumber / 2)) + 1)):
        if (random.randrange(0, 200) > currentRoundNumber * 2):
            newSlime = GreenSlimeEnemy()
            newSlime.rect.x = random.randint(0, 40 * 50)
            newSlime.rect.y = random.randint(0, 44 * 50)
            drawsList.add(newSlime)
            entitiesList.add(newSlime)
        else:
            newSlime = OrangeSlimeEnemy()
            newSlime.rect.x = random.randint(0, 40 * 50)
            newSlime.rect.y = random.randint(0, 44 * 50)
            drawsList.add(newSlime)
            entitiesList.add(newSlime)
        

def UpdateScreen():     #A method for drawing everything to the screen
    screen.fill(BLACK)

    textFont = pygame.font.Font(None, 60)
    
    if (gameState == GAMESTATE_TITLE):
        screen.blit(titleScreenBackground, (0, 0))

        titleFont = pygame.font.Font(None, 180)

        titleText = titleFont.render("FOREST GUARD", 1, BLACK)
        screen.blit(titleText, ((size[0] / 2) - 480, 40))
        
        startText = textFont.render("Press SPACE to start", 1, BLACK)
        screen.blit(startText, ((size[0] / 2) - 220, 600))

        rulesText = textFont.render("Press R to view the rules", 1, BLACK)
        screen.blit(rulesText, ((size[0] / 2) - 240, 640))

        if (showingRules):
            rectX = 50
            rectY = 295
            textSpacing = 40
            pygame.draw.rect(screen, (255, 255, 161), pygame.Rect(rectX, rectY, 1100, 160))
            
            wasdText = textFont.render("Use WASD to move", 1, BLACK)
            screen.blit(wasdText, (rectX + 20, rectY + 20))

            mouseText = textFont.render("Use left-click to swing your sword", 1, BLACK)
            screen.blit(mouseText, (rectX + 20, rectY + 20 + textSpacing))

            goalText = textFont.render("Goal: Survive for as long as you can and kill enemies", 1, BLACK)
            screen.blit(goalText, (rectX + 20, rectY + 20 + textSpacing * 2))
        
    elif (gameState == GAMESTATE_PLAYING):
        cameraPosition = [player.rect.x, player.rect.y]

        if (cameraPosition[0] < 100):
            cameraPosition[0] = 100
            
        if (cameraPosition[0] > (mapWidth * 50) - 100):
            cameraPosition[0] = (mapWidth * 50) - 100
        
        
        row = 0
        column = -1
        for i in range(len(mapTilesList) - 1):      #Drawing the map
            column += 1
            if (column > mapHeight):
                row += 1
                column = 0

            tilePosX = row * 50
            tilePosY = column * 50
            screen.blit(mapTilesList[i], (tilePosX - cameraPosition[0], tilePosY - cameraPosition[1]))


        drawsListCopy = list(drawsList)
        #ySortedDraws = []
        ySortedDraws = drawsListCopy
        #storedYPositions = []       #List of the Y positions
        #for draw in drawsList:
        #    storedYPositions.append(draw.rect.y)

        #print("Before: " + str(storedYPositions))
        #for i in range(0, len(storedYPositions)):
        #    recordedIndex = -1
        #    currentLowest = 99999
        #    for x in range(0, len(storedYPositions)):
        #        if (storedYPositions[x] <= currentLowest):
        #            recordedIndex = x
        #            currentLowest = storedYPositions[x]

        #    ySortedDraws.append(drawsListCopy[recordedIndex])
        #    lowestYPos = storedYPositions[recordedIndex]

        #for draw in ySortedDraws:
        #    storedYPositions.append(draw.rect.y)

        #print("After: " + str(storedYPositions))

        for draw in ySortedDraws:
            draw.Draw(screen, cameraPosition)

        healthText = textFont.render("Health: " + str(player.health), 1, BLACK)
        screen.blit(healthText, (0, 0))
        
        roundText = textFont.render("Round: " + str(currentRoundNumber + 1), 1, BLACK)
        screen.blit(roundText, (0, 40))

        enemiesLeftText = textFont.render("Enemies Left: " + str(len(entitiesList) - 1), 1, BLACK)
        screen.blit(enemiesLeftText, (0, 80))
        
    elif (gameState == GAMESTATE_GAMEOVER):
        screen.blit(gameOverBackground, (0, 0))

        roundNumberText = textFont.render("Amount of Rounds Passed:" + str(currentRoundNumber), 1, BLACK)
        screen.blit(roundNumberText, ((size[0] / 2) - 240, 185))

        enemiesKilledText = textFont.render("Amount of Enemies Killed:" + str(player.enemiesKilled), 1, BLACK)
        screen.blit(enemiesKilledText, ((size[0] / 2) - 236, 225))

        restartText = textFont.render("Press SPACE to restart", 1, BLACK)
        screen.blit(restartText, ((size[0] / 2) - 220, 600))
        
        quitText = textFont.render("Press ESCAPE to quit", 1, BLACK)
        screen.blit(quitText, ((size[0] / 2) - 212, 640))

    pygame.display.update()

def UpdateEntities():       #A method for updating everything on the screen
    for entity in entitiesList:
        if (hasattr(entity, "enemy")):
            entity.UpdateAI(player)

            if (player.rect.colliderect(entity.rect) and player.immunityTimer <= 0):
                player.health -= 1
                player.immunityTimer = 30
            if (entity.immunityTimer <= 0 and player.swordSwingTimer > 0 and player.swordRect.colliderect(entity.rect)):
                slimeHitSound.play()
                entity.Hurt()
                if (entity.health <= 0):
                    player.enemiesKilled += 1
                    entitiesList.remove(entity)
                    drawsList.remove(entity)
        else:
            entity.Update()

def UpdateCamera():
    cameraPosition = [player.rect.x - size[0] / 2, player.rect.y - size[1] / 2]
    
    if (cameraPosition[0] < size[0] / 2):
        cameraPosition[0] = size[0] / 2
        
    if (cameraPosition[0] > (mapWidth * 50) - (size[0] / 2)):
        cameraPosition[0] = (mapWidth * 50) - (size[0] / 2)
        
    if (cameraPosition[1] > size[1] / 2):
        cameraPosition[1] = size[1] / 2
        
    if (cameraPosition[1] < (mapHeight * 50) - (size[1] / 2)):
        cameraPosition[1] = (mapHeight * 50) - (size[1] / 2)

    #print(cameraPosition)


def GenerateMap():
    for i in range(mapWidth * mapHeight):
        if (random.randint(0, 101) <= 98):
            mapTilesList.append(grassTile1)
        else:
            mapTilesList.append(grassTile2)


    for i in range(0, random.randint(35, 62)):
        treePosX = random.randint(0, mapWidth * 50)
        treePosY = random.randint(0, mapHeight * 50)
        tree = Tree(treePosX, treePosY)
        drawsList.add(tree)

def PlayMusic(musicType):
    if (musicType == MUSIC_TITLE):
        pygame.mixer.music.load("Sounds/Music/TitleSong.mp3")
    elif (musicType == MUSIC_GAME):
        pygame.mixer.music.load("Sounds/Music/Main_Chill_Action.mp3")
    pygame.mixer.music.rewind()
    pygame.mixer.music.play(-1)

PlayMusic(MUSIC_TITLE)

while gameRunning:      #The Main Loop
    for event in pygame.event.get():
        if (event.type == pygame.KEYDOWN):
            if (event.key == pygame.K_ESCAPE):
                gameRunning = False
            if (gameState == GAMESTATE_TITLE):
                if (event.key == pygame.K_SPACE):
                    InitializeGame()
                    gameState = GAMESTATE_PLAYING
                if (event.key == pygame.K_r):
                    showingRules = not showingRules
            if (gameState == GAMESTATE_GAMEOVER):
                if (event.key == pygame.K_SPACE):
                    InitializeGame()
                    currentRoundNumber = 0
                    gameState = GAMESTATE_PLAYING
            
    if (gameState == GAMESTATE_PLAYING):
        UpdateCamera()
        UpdateEntities()
        
        if (len(entitiesList) == 1):
            currentRoundNumber += 1
            StartNewRound()

        if (player.health <= 0):
            gameState = GAMESTATE_GAMEOVER

    UpdateScreen()
    pygame.display.flip()
    clock.tick(60)

pygame.quit()
