    function serialize (o, ident)
      if (ident == nil) then
          ident = " "
      end
      if (o == nil) then
        io.write("nillllll")
      elseif type(o) == "number" then
        io.write(o)
      elseif type(o) == "string" then
        io.write(string.format("%q", o))
      elseif type(o) == "boolean" then
        io.write(string.format("%s", o))
      elseif type(o) == "table" then
        io.write("{\n")
        for k,v in pairs(o) do
           io.write(ident.."   ")
		   if(type(k) == "string") then
			io.write(string.format("[%q] = ", k))
		   elseif(type(k) == "number") then
			io.write("[", k, "] = ")
		   end          
          serialize(v, ident.."   ")
          io.write(",\n")
        end
        io.write(ident.."}")
      else
        error("cannot serialize a " .. type(o))
      end
    end
    
    mission = {
        	["groundControl"] = 
	{
		["passwords"] = 
		{
			["artillery_commander"] = {
			    ["something"] = {
			        ["anothertable"] = {
			            ["foo"] = "bar",
			            ["imanil"] = nil,
			            ["fizz"] = "buzz",
			            [1] = "something"
			        }
			    }
			},
			["instructor"] = {},
			["observer"] = {},
			["forward_observer"] = {},
		}, -- end of ["passwords"]
		["roles"] = 
		{
			["artillery_commander"] = 
			{
				["neutrals"] = 0,
				["blue"] = 0,
				["red"] = 0,
			}, -- end of ["artillery_commander"]
			["instructor"] = 
			{
				["neutrals"] = 1,
				["blue"] = 0,
				["red"] = 0,
			}, -- end of ["instructor"]
			["observer"] = 
			{
				["neutrals"] = 1,
				["blue"] = 1,
				["red"] = 1,
			}, -- end of ["observer"]
			["forward_observer"] = 
			{
				["neutrals"] = 0,
				["blue"] = 0,
				["red"] = 0,
			}, -- end of ["forward_observer"]
		}, -- end of ["roles"]
		["isPilotControlVehicles"] = false,
	}
    }
    
    print(serialize(mission, "   "))