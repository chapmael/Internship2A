%% Importation des fichiers
% Importation des fichiers PCD
clear all 
close all 
clc

FilesName=dir('DataSet\2\PCD\');
nbFilePCD=length(find([FilesName.isdir]==0));
ListFilePCD=strings(nbFilePCD);
for i=3:nbFilePCD+2
    ListFilePCD(i-2)=strcat('DataSet\2\PCD\',FilesName(i,1).name);
end

% Importation des fichiers Labeling

FilesName=dir('DataSet\2\Labeling\');
nbFileLabel=length(find([FilesName.isdir]==0));
ListFileLabel=strings(nbFileLabel);
for i=3:nbFileLabel+2
    ListFileLabel(i-2)=strcat('DataSet\2\Labeling\',FilesName(i,1).name);
end


%% Rotations nécessaires pour la bonne visualisation

for i=1:nbFilePCD
    id=fopen(ListFilePCD(i),'r');
    for k=1:8
        line = fgetl(id);
    end
    Line=strsplit(line,' ');
    c=[str2double(strrep(Line(2),',','')) str2double(Line(4)) str2double(strrep(Line(3),',',''))];
    t=[str2double(strrep(Line(6),',','')) str2double(strrep(Line(8),')','')) str2double(strrep(Line(7),',',''))];
    if i==1
        Cam=[c];
        Target=[t];
    else
        Cam=[Cam;c];
        Target=[Target;t];
    end
    fclose(id);
end


%% Visualisation sans bounding box sans enregistrement
%%%%%%%%%%%%%%%%
% uses this code after launching the two previous scripts.
 
clc
close all
f=figure();
f.Position(:)=1.0e+03 *[0.0010    0.0490    1.7067    0.9460];

for i=1:nbFilePCD
    ptCloud = pcread(ListFilePCD(i));
    coord=[ptCloud.Location(:,1),ptCloud.Location(:,3),ptCloud.Location(:,2)];
    ptcloud = pointCloud(coord);
    pcshow(ptcloud);
    
    campos(Cam(i,:));
    camtarget(Target(i,:));
    
    axis off;
    pause(0.01);
end


%% Ajout des bounding Boxs
%Recupération des coordonnées
delimeters = {' ','(',')',','};

for k=1:234
    d = readtable(ListFileLabel(k),'Delimiter',delimeters);
    d = removevars(d,{'Var1','Var3','Var5','Var7','Var8','Var10','Var12','Var14','Var15','Var17','Var19','Var21'});
    d = convertvars(d,{'Var2','Var4','Var6','Var9','Var11','Var13','Var16','Var18','Var20'},'double');
    CenterTab=d{:,4:6};
    SizeTab=d{:,7:9};
    NameTab=d{:,10};
    if k==1
        CenterAll=CenterTab;
        SizeAll=SizeTab;
        NameAll=NameTab;
    else
        CenterAll(:,:,k)=CenterTab;
        SizeAll(:,:,k)=SizeTab;
        NameAll(:,:,k)=NameTab;
    end
    
end



