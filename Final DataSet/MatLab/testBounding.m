i=232;
ptCloud = pcread(ListFilePCD(i));
coord=[ptCloud.Location(:,1),ptCloud.Location(:,3),ptCloud.Location(:,2)];
ptcloud = pointCloud(coord);
BoxCenter=CenterAll(:,:,i);
BoxSize=SizeAll(:,:,i);
BoxName=NameAll(:,:,i);



%x=[BoxCenter(1)+BoxSize(1)/2,BoxCenter(1)+BoxSize(1)/2];
%y=[BoxCenter(3)+BoxSize(3)/2,BoxCenter(3)+BoxSize(3)/2];
%z=[BoxCenter(2)+BoxSize(2)/2,BoxCenter(2)-BoxSize(1)/2];
 
pcshow(ptcloud);
for k=1:length(BoxCenter)
    if not(strcmp(char(BoxName(k)),'Terrain')||strcmp(char(BoxName(k)),'Road'))
        [X,Y,Z]=SetBox(BoxCenter(k,:),BoxSize(k,:));
    
        for q=1:12
            hold on;
            plot3(X(2*(q-1)+1:2*(q-1)+2),Y(2*(q-1)+1:2*(q-1)+2),Z(2*(q-1)+1:2*(q-1)+2),'Color','w');
           
        end
    end
end

campos(Cam(i,:));
camtarget(Target(i,:));  
axis off;