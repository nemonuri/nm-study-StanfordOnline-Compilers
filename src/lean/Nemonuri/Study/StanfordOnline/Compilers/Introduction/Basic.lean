module

public import Cslib.Foundations.Semantics.LTS.Basic

public section public_s
@[expose] section expose_s

set_option autoImplicit false

namespace Nemonuri.Study.StanfordOnline.Compilers

universe u v

structure State (St: Type u) where val: St

structure Label (La: Type v) where val: La

def LTS (St: Type u) (La: Type v) := Cslib.LTS (State St) (Label La)

end Nemonuri.Study.StanfordOnline.Compilers

end expose_s
end public_s
