function [Xf,Yf,Zf]=SetBox(BoxCenter,BoxSize)
    for i=1:2
        for k=1:2
            x=[BoxCenter(1)+((-1)^i)*BoxSize(1)/2,BoxCenter(1)+((-1)^i)*BoxSize(1)/2,BoxCenter(1)+BoxSize(1)/2,BoxCenter(1)-BoxSize(1)/2,BoxCenter(1)+((-1)^i)*BoxSize(1)/2,BoxCenter(1)+((-1)^i)*BoxSize(1)/2];
            y=[BoxCenter(3)+BoxSize(3)/2,BoxCenter(3)-BoxSize(3)/2,BoxCenter(3)+((-1)^i)*BoxSize(3)/2,BoxCenter(3)+((-1)^i)*BoxSize(3)/2,BoxCenter(3)+((-1)^k)*BoxSize(3)/2,BoxCenter(3)+((-1)^k)*BoxSize(3)/2];
            z=[BoxCenter(2)+((-1)^k)*BoxSize(2)/2,BoxCenter(2)+((-1)^k)*BoxSize(2)/2,BoxCenter(2)+((-1)^k)*BoxSize(2)/2,BoxCenter(2)+((-1)^k)*BoxSize(2)/2,BoxCenter(2)+BoxSize(2)/2,BoxCenter(2)-BoxSize(2)/2,];
        
            if (i==1)&&(k==1)
                X=x;
                Y=y;
                Z=z;
            else
                X=[X,x];
                Y=[Y,y];
                Z=[Z,z];
            end
        end
    end
    Xf=X;
    Yf=Y;
    Zf=Z;
end