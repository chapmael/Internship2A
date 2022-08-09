class PreWorkAI:
    def __init__(self):
        #self.m_path=r'C:\Users\chapm\Documents\unity\construction_environment_Test\Assets\Scripts\Construction_env_simulation\CommunicationPython\TestFile\Test.txt'
        self.m_path=r'C:\Users\chapm\Documents\unity\save_point\DefaultName.pcd'

    def ReturnLastLine(self) :
        file= open(self.m_path)
        lines = file.readlines()
        file.close()
        for l in lines :
            line=l
        return line
