import subprocess as sub
from time import sleep
import sys

def ev_Shutdown():
    sub.call('start shutdown -s -t 1', shell=True)

#def ev_SearchPO():
    #sub.call('start http://www.google.com', shell=True)
    #sleep(0.5)
    #auto.hotkey('ctrl', 'e')
    #sleep(0.1)
    #auto.press('backspace')
    #sleep(0.1)
    #auto.write('po')

def ev_RickRoll():
    sub.call('start "" "https://www.youtube.com/watch?v=dQw4w9WgXcQ"', shell=True)

def ev_Chamba():
    sub.call('start https://www.linkedin.com/', shell=True)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        action = sys.argv[1]
        
        if action == "shutdown":
            ev_Shutdown()
        #elif action == "searchPO":
            #ev_SearchPO()
        elif action == "rickroll":
            ev_RickRoll()
        elif action == "chamba":
            ev_Chamba()
        else:
            print(f"No existe la acción: {action}")
    else:
        print("Especifica una acción nmms Kori")